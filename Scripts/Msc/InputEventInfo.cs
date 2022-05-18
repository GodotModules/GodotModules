using Godot;

namespace GodotModules
{
    public enum InputEventType
    {
        Key,
        MouseButton,
        JoypadButton,
    }

    public record struct InputEventInfo 
    {
        // Type
        public InputEventType Type { get; private set; }

        // InputEvent
        public int Device { get; private set;  }

        // InputEventWithModifiers
        public bool Alt { get; private set;  }
        public bool Command { get; private set;  }
        public bool Control { get; private set;  }
        public bool Meta { get; private set;  }
        public bool Shift { get; private set;  }

        // InputEventKey
        public uint Scancode { get; private set;  }
        public uint PhysicalScancode { get; private set;  }
        public uint Unicode { get; private set;  }

        // InputEventMouseButton
        public int ButtonMask { get; private set; }

        // InputEventJoypadButton
        public int ButtonIndex { get; private set; }

        public bool IsEventKey => Type == InputEventType.Key;
        public bool IsEventMouseButton => Type == InputEventType.MouseButton;
        public bool IsEventJoypadButton => Type == InputEventType.JoypadButton;

        public InputEventKey ToEventKey()
        {
            if (Type != InputEventType.Key)
                throw new InvalidOperationException("This InputEventInfo is not a Key event.");
            
            return new InputEventKey
            {
                Scancode = Scancode,
                PhysicalScancode = PhysicalScancode,
                Unicode = Unicode,
                Alt = Alt,
                Shift = Shift,
                Control = Control,
                Meta = Meta,
                Command = Command,
                Device = Device,
            };
        }

        public InputEventMouseButton ToEventMouseButton()
        {
            if (Type != InputEventType.MouseButton)
                throw new InvalidOperationException("This InputEventInfo is not a MouseButton event.");
            
            return new InputEventMouseButton
            {
                ButtonMask = ButtonMask,
                Alt = Alt,
                Shift = Shift,
                Control = Control,
                Meta = Meta,
                Command = Command,
                Device = Device,
            };
        }

        public InputEventJoypadButton ToEventJoypadButton()
        {
            if (Type != InputEventType.JoypadButton)
                throw new InvalidOperationException("This InputEventInfo is not a JoypadButton event.");
            
            return new InputEventJoypadButton
            {
                ButtonIndex = ButtonIndex,
                Device = Device,
            };
        }

        public InputEvent ToEvent()
        {
            if (Type == InputEventType.Key)
                return ToEventKey();
            
            if (Type == InputEventType.MouseButton)
                return ToEventMouseButton();
            
            if (Type == InputEventType.JoypadButton)
                return ToEventJoypadButton();
            
            throw new InvalidOperationException("This InputEventInfo is not a Key, MouseButton or JoypadButton event.");
        }

        public static InputEventInfo From(InputEventKey k) => new InputEventInfo
        {
            Type = InputEventType.Key,
            Device = k.Device,
            Alt = k.Alt,
            Command = k.Command,
            Control = k.Control,
            Meta = k.Meta,
            Shift = k.Shift,
            Scancode = k.Scancode,
            PhysicalScancode = k.PhysicalScancode,
            Unicode = k.Unicode,
        };

        public static InputEventInfo From(InputEventMouseButton m) => new InputEventInfo
        {
            Type = InputEventType.MouseButton,
            Device = m.Device,
            Alt = m.Alt,
            Command = m.Command,
            Control = m.Control,
            Meta = m.Meta,
            Shift = m.Shift,
            ButtonMask = m.ButtonMask,
        };

        public static InputEventInfo From(InputEventJoypadButton j) => new InputEventInfo
        {
            Type = InputEventType.JoypadButton,
            Device = j.Device,
            ButtonIndex = j.ButtonIndex,
        };

        public static InputEventInfo? TryFrom(InputEvent e)
        {
            if (e is InputEventKey k)
                return From(k);
            
            if (e is InputEventMouseButton m)
                return From(m);
            
            if (e is InputEventJoypadButton j)
                return From(j);
            
            return null;
        }
    }
} 