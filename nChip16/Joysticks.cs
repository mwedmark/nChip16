using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nChip16
{
    //Bit[0] - Up
    //Bit[1] - Down
    //Bit[2] - Left
    //Bit[3] - Right
    //Bit[4] - Select
    //Bit[5] - Start
    //Bit[6] - A
    //Bit[7] - B
    //Bit[8 - 15] - Unused (Always zero).
    public class Joystick
    {
        public Joystick() {}

        public int JoystickNumber { get; set; }
        public JoystickMoves Change { get; set; }
        public override string ToString()
        {
            return string.Format("Joystick: {0}, Move: {1}", JoystickNumber, Change);
        }
    }

    public enum JoystickMoves
    {
        NotMapped,
        Up,
        Down,
        Left,
        Right,
        Select,
        Start,
        A,
        B
    };
}
