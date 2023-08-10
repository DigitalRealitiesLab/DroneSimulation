using System;

namespace Support.UserInput.InputTypes {
    [Flags]
    public enum XBoxControllerInput : short {
        LStick = 1,
        RStick = 2,
        LT = 4,
        RT = 8,
        A = 16,
        B = 32,
        X = 64,
        Y = 128,
        LB = 256,
        RB = 512,
        Menu = 1024
    }
}