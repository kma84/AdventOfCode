using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace day2
{
    class Program
    {

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


        static void Main(string[] args)
        {
            List<Movimiento> Movimientos = GetMovimientos();

            Puzle1(Movimientos);
            Puzle2(Movimientos);
        }


        private static void Puzle1(List<Movimiento> movimientos)
        {
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

            Console.WriteLine($"Puzle 1: Profundidad: {profundidad}, posición horizontal: {posicionHorizontal}, producto: {profundidad * posicionHorizontal}");
        }


        private static void Puzle2(List<Movimiento> movimientos)
        {
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

            Console.WriteLine($"Puzle 2: Profundidad: {profundidad}, posición horizontal: {posicionHorizontal}, producto: {profundidad * posicionHorizontal}");
        }


        private static List<Movimiento> GetMovimientos()
        {
            string input = 
                File.ReadAllText(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "inputs" + Path.DirectorySeparatorChar + "input.txt");

            string[] movs = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            List<Movimiento> Movimientos = new();

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
            "up"        => TipoMovimientoSubmarino.UP,
            "down"      => TipoMovimientoSubmarino.DOWN,
            "forward"   => TipoMovimientoSubmarino.FORWARD,
            _           => throw new NotImplementedException()
        };

    }
}
