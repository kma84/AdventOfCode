
using System.Text;

const int GRID_SIZE = 10;
const int OCTOPUS_MAX_ENERGY = 9;


Octopus[,] grid = GetGrid("input.txt"); //step0.txt

MostrarGrid(grid, "step0");

int numFlashes = 0;
int numPasos = 100;
for (int i = 1; i <= numPasos; i++)
{
    numFlashes += AvanzarGrid(grid);
    MostrarGrid(grid, "step" + i);
}

Console.WriteLine("Número de flashes: " + numFlashes);

while (!FlashSimultaneo(grid))
{
    AvanzarGrid(grid);
    numPasos++;
}

Console.WriteLine("Flash simultáneo en el paso: " + numPasos);


bool FlashSimultaneo(Octopus[,] grid)
{
    for (int y = 0; y < GRID_SIZE; y++)
    {
        for (int x = 0; x < GRID_SIZE; x++)
        {
            if(grid[y, x].Energy > 0)
                return false;
        }
    }

    return true;
}


int AvanzarGrid(Octopus[,] grid)
{
    int numFlashes = 0;

    for (int y = 0; y < GRID_SIZE; y++)
    {
        for (int x = 0; x < GRID_SIZE; x++)
        {
            grid[y, x].Energy++;
        }
    }

    while (PulposPorFlashear(grid))
    {
        for (int y = 0; y < GRID_SIZE; y++)
        {
            for (int x = 0; x < GRID_SIZE; x++)
            {
                if (!grid[y, x].FlashEnEsteTurno && grid[y, x].Energy > OCTOPUS_MAX_ENERGY)
                {
                    numFlashes++;
                    grid[y, x].Energy = 0;
                    grid[y, x].FlashEnEsteTurno = true;

                    // Aumentamos la energía de los pulpos adyacentes
                    if (y - 1 >= 0 && x - 1 >= 0 && !grid[y - 1, x - 1].FlashEnEsteTurno)
                        grid[y - 1, x - 1].Energy++;

                    if (y - 1 >= 0 && !grid[y - 1, x].FlashEnEsteTurno)
                        grid[y - 1, x].Energy++;

                    if (y - 1 >= 0 && x + 1 < GRID_SIZE && !grid[y - 1, x + 1].FlashEnEsteTurno)
                        grid[y - 1, x + 1].Energy++;

                    if (x - 1 >= 0 && !grid[y, x - 1].FlashEnEsteTurno)
                        grid[y, x - 1].Energy++;

                    if (x + 1 < GRID_SIZE && !grid[y, x + 1].FlashEnEsteTurno)
                        grid[y, x + 1].Energy++;

                    if (y + 1 < GRID_SIZE && x - 1 >= 0 && !grid[y + 1, x - 1].FlashEnEsteTurno)
                        grid[y + 1, x - 1].Energy++;

                    if (y + 1 < GRID_SIZE && !grid[y + 1, x].FlashEnEsteTurno)
                        grid[y + 1, x].Energy++;

                    if (y + 1 < GRID_SIZE && x + 1 < GRID_SIZE && !grid[y + 1, x + 1].FlashEnEsteTurno)
                        grid[y + 1, x + 1].Energy++;
                }
            }
        }
    }

    LimpiarFlashes(grid);

    return numFlashes;
}


bool PulposPorFlashear(Octopus[,] grid)
{
    for (int y = 0; y < GRID_SIZE; y++)
    {
        for (int x = 0; x < GRID_SIZE; x++)
        {
            if(grid[y, x].Energy > OCTOPUS_MAX_ENERGY)
                return true;
        }
    }

    return false;
}


void LimpiarFlashes(Octopus[,] grid)
{
    for (int y = 0; y < GRID_SIZE; y++)
    {
        for (int x = 0; x < GRID_SIZE; x++)
        {
            grid[y, x].FlashEnEsteTurno = false;
        }
    }
}


Octopus[,] GetGrid(string filename)
{
    string input =
        File.ReadAllText(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "inputs" + Path.DirectorySeparatorChar + filename);

    string[] filas = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);

    Octopus[,] grid = new Octopus[GRID_SIZE, GRID_SIZE];

    for (int y = 0; y < GRID_SIZE; y++)
    {
        for (int x = 0; x < GRID_SIZE; x++)
        {
            grid[y, x] = new Octopus { Energy = (int)char.GetNumericValue(filas[y][x]) }; 
        }
    }

    return grid;
}


void MostrarGrid(Octopus[,] grid, string titulo)
{
    StringBuilder sb = new();

    for (int y = 0; y < GRID_SIZE; y++)
    {
        for (int x = 0; x < GRID_SIZE; x++)
        {
            sb.Append(grid[y, x].Energy);
        }

        sb.AppendLine();
    }

    Console.WriteLine(titulo);
    Console.WriteLine(sb.ToString());
}


class Octopus
{
    public int Energy { get; set; }

    public bool FlashEnEsteTurno { get; set; } = false;
}