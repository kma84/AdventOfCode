using AdventOfCode.Core;
using AdventOfCode.Core.Interfaces;
using AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Year2021.Day02
{
    [Problem(Year = 2021, Day = 2, ProblemName = "Dive!")]
    internal class Problem : IProblem
    {
        public bool Debug { get; set; } = false;


        public string Part1(string input)
        {
            List<Movimiento> movimientos = GetMovimientos(input);

            int posicionHorizontal = 0;
            int profundidad = 0;

            foreach (Movimiento movimiento in movimientos)
            {
                switch (movimiento.TipoMovimiento)
                {
                    case TipoMovimientoSubmarino.UP:
                        profundidad -= movimiento.Valor;
                        break;

                    case TipoMovimientoSubmarino.DOWN:
                        profundidad += movimiento.Valor;
                        break;

                    case TipoMovimientoSubmarino.FORWARD:
                    default:
                        posicionHorizontal += movimiento.Valor;
                        break;
                }
            }

            int producto = profundidad * posicionHorizontal;

            if (Debug)
                Console.WriteLine($"Puzle 1: Profundidad: {profundidad}, posición horizontal: {posicionHorizontal}, producto: {producto}");

            return producto.ToString();
        }

        public string Part2(string input)
        {
            List<Movimiento> movimientos = GetMovimientos(input);

            int posicionHorizontal = 0;
            int profundidad = 0;
            int precision = 0;

            foreach (Movimiento movimiento in movimientos)
            {
                switch (movimiento.TipoMovimiento)
                {
                    case TipoMovimientoSubmarino.UP:
                        precision -= movimiento.Valor;
                        break;

                    case TipoMovimientoSubmarino.DOWN:
                        precision += movimiento.Valor;
                        break;

                    case TipoMovimientoSubmarino.FORWARD:
                    default:
                        posicionHorizontal += movimiento.Valor;
                        profundidad += movimiento.Valor * precision;
                        break;
                }
            }

            int producto = profundidad * posicionHorizontal;

            if (Debug)
                Console.WriteLine($"Puzle 2: Profundidad: {profundidad}, posición horizontal: {posicionHorizontal}, producto: {producto}");

            return producto.ToString();
        }


        private static List<Movimiento> GetMovimientos(string input)
        {
            string[] movs = input.GetLines();

            List<Movimiento> Movimientos = [];

            foreach (string mov in movs)
            {
                string[] movSplit = mov.Split();

                Movimientos.Add(new Movimiento
                {
                    TipoMovimiento = GetTipoMovimiento(movSplit[0]),
                    Valor = int.Parse(movSplit[1])
                });
            }

            return Movimientos;
        }

        private static TipoMovimientoSubmarino GetTipoMovimiento(string movStr) => movStr switch
        {
            "up" => TipoMovimientoSubmarino.UP,
            "down" => TipoMovimientoSubmarino.DOWN,
            "forward" => TipoMovimientoSubmarino.FORWARD,
            _ => throw new NotImplementedException()
        };


        public enum TipoMovimientoSubmarino
        {
            UP,
            DOWN,
            FORWARD
        }


        public class Movimiento
        {
            public TipoMovimientoSubmarino TipoMovimiento { get; set; }
            public int Valor { get; set; }
        }

    }
}