using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSortingScript.Display
{
    public static class SpecialPrinting
    {
        public static void PrintColored(string text, ConsoleColor textColor, params object[] interpolatedValues)
        {
            int currentIndex = 0;

            foreach ( var interpolatedValue in interpolatedValues )
            {
                string valueString = interpolatedValue.ToString()!;
                int valueIndex = text.IndexOf(valueString, currentIndex);

                if(valueIndex >= 0 )
                {
                    // Print the part of the message before the interpolated value
                    Console.ForegroundColor = textColor;
                    Console.Write(text.Substring(currentIndex, valueIndex - currentIndex));

                    // print the interpolated value in different color
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(valueString);
                    Console.ResetColor();
                    
                    // Update the current index to continue parsing the rest of the message
                    currentIndex = valueIndex + valueString.Length;
                }
            }

            // print the remaining part of the message after the last interpolated value
            // this will also cover if there are not interpolated values
            Console.ForegroundColor = textColor;
            
            if(currentIndex < text.Length)
                Console.Write(text.Substring(currentIndex));

            Console.ResetColor();

            Console.WriteLine();
        }
    }
}
