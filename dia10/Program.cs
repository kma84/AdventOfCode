
List<char> caracteresApertura = new() { '(', '[', '{', '<' };
List<char> caracteresCierre = new() { ')', ']', '}', '>' };


Console.WriteLine("Puntuación del testInput: " + GetInput("testInput.txt").Select(GetPuntuacionFila).Sum());
Console.WriteLine("Puntuación del input: " + GetInput("input.txt").Select(GetPuntuacionFila).Sum());

List<string> lineasIncompletas = GetInput("input.txt").Where(l => GetPuntuacionFila(l) == 0).ToList();

List<long> puntuaciones = lineasIncompletas.Select(CompletarFila).OrderBy(n => n).ToList();
Console.WriteLine("Puntuación al completar las líneas del testInput: " + puntuaciones[puntuaciones.Count / 2]);


long CompletarFila(string fila)
{
    Dictionary<char, int> puntuaciones = new() {
        { ')', 1 },
        { ']', 2 },
        { '}', 3 },
        { '>', 4 }
    };

    Stack<char> pila = new();
    string cierreFila = string.Empty;
    long result = 0;

    foreach (char c in fila)
    {
        if (caracteresApertura.Contains(c))
        {
            pila.Push(c);
        }
        else if (pila.Peek() == caracteresApertura[caracteresCierre.IndexOf(c)])
        {
            pila.Pop();
        }        
    }

    while (pila.TryPop(out char lastChar))
    {
        cierreFila += caracteresCierre[caracteresApertura.IndexOf(lastChar)];
    }

    foreach (char c in cierreFila)
    {
        result = result * 5 + puntuaciones[c];
    }

    return result;
}


int GetPuntuacionFila(string fila)
{
    Dictionary<char, int> puntuaciones = new() {
        { ')', 3 },
        { ']', 57 },
        { '}', 1197 },
        { '>', 25137 }
    };

    Stack<char> pila = new();

    foreach (char c in fila)
    {
        if (caracteresApertura.Contains(c))
        {
            pila.Push(c);
        }
        else
        {
            // es un caracter de cierre
            if (pila.TryPop(out char lastChar))
            {
                if (lastChar != caracteresApertura[caracteresCierre.IndexOf(c)])
                {
                    return puntuaciones[c];
                }
            }
            else
            {
                // caso donde el primer char ya es erróneo
                return puntuaciones[c];
            }
        }
    }

    return 0;
}


string[] GetInput(string filename)
{
    string input =
        File.ReadAllText(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "inputs" + Path.DirectorySeparatorChar + filename);

    return input.Split('\n', StringSplitOptions.RemoveEmptyEntries);
}
