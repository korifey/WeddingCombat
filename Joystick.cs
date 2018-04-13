//using System.Collections.Generic;
//using SlimDX.DirectInput;
//
//namespace WeddingCombat
//{
//    public class Joystick
//    {
//        public virtual IList<GamepadDevice> Available()
//        {
//            IList<GamepadDevice> result = new List<GamepadDevice>();
//            DirectInput dinput = new DirectInput();
//            foreach (DeviceInstance di in dinput.GetDevices(DeviceClass.GameController,
//                DeviceEnumerationFlags.AttachedOnly))
//            {
//                GamepadDevice dev = new GamepadDevice();
//                dev.Guid = di.InstanceGuid;
//                dev.Name = di.InstanceName;
//                result.Add(dev);
//            }
//
//            return result;
//        }
//    }
//
//    private void acquire(System.Windows.Forms.Form parent)
//    {
//    DirectInput dinput = new DirectInput();
//
//    pad = new Joystick(dinput, this.Device.Guid);
//        foreach (DeviceObjectInstance doi in pad.GetObjects(ObjectDeviceType.Axis))
//    {
//        pad.GetObjectPropertiesById((int) doi.ObjectType).SetRange(-5000, 5000);
//    }
//
//    pad.Properties.AxisMode = DeviceAxisMode.Absolute;
//    pad.SetCooperativeLevel(parent, (CooperativeLevel.Nonexclusive | CooperativeLevel.Background));
//    pad.Acquire();
//    }
//
//    void X() {
//
//    JoystickState state = new JoystickState();
//
//        if (pad.Poll().IsFailure)
//    {
//        result.Disconnect = true;
//        return result;
//    }
//
//    if (pad.GetCurrentState(ref state).IsFailure)
//    {
//    result.Disconnect = true;
//    return result;
//    }
//
//    result.X = state.X / 5000.0f;
//    result.Y = state.Y / 5000.0f;
//    int ispressed = 0;
//    bool[] buttons = state.GetButtons();
//}
//
//static void Main()
//{
//// Initialize DirectInput
//var directInput = new DirectInput();
//
//// Find a Joystick Guid
//var joystickGuid = Guid.Empty;
//
//    foreach (var deviceInstance in directInput.GetDevices(DeviceType.Gamepad, 
//    DeviceEnumerationFlags.AllDevices))
//joystickGuid = deviceInstance.InstanceGuid;
//
//// If Gamepad not found, look for a Joystick
//if (joystickGuid == Guid.Empty)
//foreach (var deviceInstance in directInput.GetDevices(DeviceType.Joystick, 
//    DeviceEnumerationFlags.AllDevices))
//joystickGuid = deviceInstance.InstanceGuid;
//
//// If Joystick not found, throws an error
//if (joystickGuid == Guid.Empty)
//{
//    Console.WriteLine("No joystick/Gamepad found.");
//    Console.ReadKey();
//    Environment.Exit(1);
//}
//
//// Instantiate the joystick
//var joystick = new Joystick(directInput, joystickGuid);
//
//Console.WriteLine("Found Joystick/Gamepad with GUID: {0}", joystickGuid);
//
//// Query all suported ForceFeedback effects
//var allEffects = joystick.GetEffects();
//    foreach (var effectInfo in allEffects)
//Console.WriteLine("Effect available {0}", effectInfo.Name);
//
//// Set BufferSize in order to use buffered data.
//joystick.Properties.BufferSize = 128;
//
//// Acquire the joystick
//joystick.Acquire();
//
//// Poll events from joystick
//while (true)
//{
//    joystick.Poll();
//    var datas = joystick.GetBufferedData();
//    foreach (var state in datas)
//        Console.WriteLine(state);
//}
//}
//
//}