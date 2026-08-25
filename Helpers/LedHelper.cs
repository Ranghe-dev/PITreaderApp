using System;
using System.Collections.Generic;
using System.Text;

namespace PITreaderApp.Helpers
{
    public static class LedHelper
    {
        public static string GetColourDescription(int colour)
        {
            return colour switch
            {
                0 => "⚫ Spento",    
                1 => "🔵 Blu",
                2 => "🟡 Giallo",
                3 => "🔴 Rosso",
                4 => "🟢 Verde",
                _ => $"Sconosciuto ({colour})"
            };
        }
        public static string GetFlashDescription(int flashMode)
        {
            return flashMode switch
            {
                0 => "Fisso",
                1 => "Lampeggio [1Hz]",
                _ => $"Sconosciuto ({flashMode})"
            };
        }
    }
}
