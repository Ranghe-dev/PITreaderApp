using System;
using System.Collections.Generic;
using System.Text;

namespace PITreaderApp.Services
{
    public static class EventTranslator
    {
        private static readonly Dictionary<int, string> _events = new()
        {
            { 20605, "Transponder rimosso" },
            { 20604, "Transponder rilevato" },
            { 20570, "Autenticazione riuscita" }
        };

        public static string GetDescription(int id)
        {
            return _events.TryGetValue(id, out var description)
                ? description
                : $"Evento sconosciuto ({id})";
        }
    }
}
