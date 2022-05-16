global using GodotModules;
global using GodotModules.Netcode;
global using System;
global using System.Collections.Generic;
global using System.Collections.Concurrent;
global using System.Diagnostics;
global using System.Runtime.CompilerServices;
global using System.Threading;
global using System.Text.RegularExpressions;
global using System.Threading.Tasks;
global using System.Linq;

using Godot;

namespace GodotModules
{
    public class Game : Node
    {
        [Export] public readonly NodePath NodePathAudioStreamPlayer;
        [Export] public readonly NodePath NodePathWebRequestList;
        [Export] public readonly NodePath NodePathConsole;
        [Export] public readonly NodePath NodePathErrorNotifierManager;
        [Export] public readonly NodePath NodePathPopupManager;

        private UIConsole _console;
        private OptionsManager _optionsManager;
        private TokenManager _tokenManager;
        private NetworkManager _networkManager;
        private SceneManager _sceneManager;
        private WebManager _webManager;
        private MusicManager _musicManager;
        private ErrorNotifierManager _errorNotifierManager;
        private PopupManager _popupManager;

        public override async void _Ready()
        {
            await InitManagers();
            _console = GetNode<UIConsole>(NodePathConsole);
            
            // how else would you pass this information to Logger?
            Logger.UIConsole = _console;
            Logger.ErrorNotifierManager = _errorNotifierManager;

            _musicManager.LoadTrack("Menu", "Audio/Music/Unsolicited trailer music loop edit.wav");
            _musicManager.PlayTrack("Menu");

            _networkManager.StartServer(25565, 100);
            _networkManager.StartClient("127.0.0.1", 25565);

            await _webManager.CheckConnectionAsync();
            if (_webManager.ConnectionAlive)
                await _webManager.GetExternalIpAsync();
        }

        public override async void _Process(float delta)
        {
            Logger.Update();
            await _networkManager.Update();
        }

        public override void _Input(InputEvent @event)
        {
            if (Input.IsActionJustPressed("ui_cancel"))
                if (_console.Visible)
                    _console.ToggleVisibility();
                else if (_sceneManager.EscPressed.ContainsKey(_sceneManager.CurScene))
                    _sceneManager.EscPressed[_sceneManager.CurScene]();

            if (Input.IsActionJustPressed("ui_console"))
                _console.ToggleVisibility();
        }

        public override async void _Notification(int what)
        {
            if (what == MainLoop.NotificationWmQuitRequest)
            {
                GetTree().SetAutoAcceptQuit(false);
                await Cleanup();
            }
        }

        private async Task Cleanup()
        {
            _optionsManager.SaveOptions();
            await _networkManager.Cleanup();
            _tokenManager.Cleanup();
            GetTree().Quit();
        }

        private async Task InitManagers()
        {
            var systemFileManager = new SystemFileManager();
            var hotkeyManager = new HotkeyManager(systemFileManager, new List<string>() {"UI", "Player"});
            _optionsManager = new(systemFileManager, hotkeyManager);
            _tokenManager = new();
            _webManager = new(new WebRequests(GetNode<Node>(NodePathWebRequestList)), _tokenManager, _optionsManager.Options.WebServerAddress);
            _musicManager = new(GetNode<AudioStreamPlayer>(NodePathAudioStreamPlayer), _optionsManager);
            
            _errorNotifierManager = GetNode<ErrorNotifierManager>(NodePathErrorNotifierManager);
            _popupManager = GetNode<PopupManager>(NodePathPopupManager);
            _networkManager = new(_popupManager);
            await InitSceneManager(hotkeyManager);
        }

        private async Task InitSceneManager(HotkeyManager hotkeyManager)
        {
            _sceneManager = new(this, new GodotFileManager(), hotkeyManager);

            // Pre Initialization
            _sceneManager.PreInit[Scene.Menu] = (scene) =>
            {
                var menu = (UIMenu)scene;
                menu.PreInit(_sceneManager, _networkManager, _popupManager);
            };
            _sceneManager.PreInit[Scene.Options] = (scene) =>
            {
                var options = (UIOptions)scene;
                options.PreInit(hotkeyManager, _optionsManager, _musicManager, _webManager, _sceneManager, _tokenManager);
            };
            _sceneManager.PreInit[Scene.Credits] = (scene) =>
            {
                var credits = (UICredits)scene;
                credits.PreInit(_sceneManager);
            };

            // Esc Pressed
            _sceneManager.EscPressed[Scene.Credits] = async () => await _sceneManager.ChangeScene(Scene.Menu);
            _sceneManager.EscPressed[Scene.GameServers] = async () => await _sceneManager.ChangeScene(Scene.Menu);
            _sceneManager.EscPressed[Scene.Mods] = async () => await _sceneManager.ChangeScene(Scene.Menu);
            _sceneManager.EscPressed[Scene.Options] = async () => {
                _tokenManager.Cancel("check_connection");
                await _sceneManager.ChangeScene(Scene.Menu);
            };
            _sceneManager.EscPressed[Scene.Lobby] = async () => await _sceneManager.ChangeScene(Scene.GameServers);
            _sceneManager.EscPressed[Scene.Game] = async () => await _sceneManager.ChangeScene(Scene.Menu);

            await _sceneManager.InitAsync();
        }
    }
}
