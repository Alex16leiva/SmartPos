namespace Dominio.Core
{
    public static class Utilidades
    {
        public static string ConvertirNumeroALetra(string valor)
        {
            if (!double.TryParse(valor, out double valorDecimal))
                return "CERO";

            long entero = (long)Math.Truncate(valorDecimal);
            int decimales = (int)Math.Round((valorDecimal - entero) * 100);

            string resultado = ConvertirDecimalALetra(Math.Abs(entero));

            // Formato estándar para facturación en Honduras/Latam
            string sufijoDecimal = decimales > 0
                ? $" CON {decimales:00}/100"
                : " CON 00/100";

            return $"{resultado}{sufijoDecimal}";
        }

        private static string ConvertirDecimalALetra(double n)
        {
            if (n == 0) return "CERO";
            if (n < 0) return "MENOS " + ConvertirDecimalALetra(Math.Abs(n));

            string nombre = "";

            // Billones
            if ((n / 1000000000000) >= 1)
            {
                nombre += (Math.Truncate(n / 1000000000000) == 1) ? "UN BILLON " : ConvertirDecimalALetra(Math.Floor(n / 1000000000000)) + " BILLONES ";
                n %= 1000000000000;
                if (n == 0) return nombre.Trim(); // Detener si ya no queda nada
            }

            // Millones
            if ((n / 1000000) >= 1)
            {
                nombre += (Math.Truncate(n / 1000000) == 1) ? "UN MILLON " : ConvertirDecimalALetra(Math.Floor(n / 1000000)) + " MILLONES ";
                n %= 1000000;
                if (n == 0) return nombre.Trim();
            }

            // Miles
            if ((n / 1000) >= 1)
            {
                nombre += (Math.Truncate(n / 1000) == 1) ? "MIL " : ConvertirDecimalALetra(Math.Floor(n / 1000)) + " MIL ";
                n %= 1000;
                if (n == 0) return nombre.Trim();
            }

            // Centenas
            if ((n / 100) >= 1)
            {
                if (n == 100) nombre += "CIEN";
                else if (Math.Truncate(n / 100) == 1) nombre += "CIENTO " + ConvertirDecimalALetra(n % 100);
                else if (Math.Truncate(n / 100) == 5) nombre += "QUINIENTOS " + ConvertirDecimalALetra(n % 100);
                else if (Math.Truncate(n / 100) == 7) nombre += "SETECIENTOS " + ConvertirDecimalALetra(n % 100);
                else if (Math.Truncate(n / 100) == 9) nombre += "NOVECIENTOS " + ConvertirDecimalALetra(n % 100);
                else nombre += ConvertirDecimalALetra(Math.Floor(n / 100)) + "CIENTOS " + ConvertirDecimalALetra(n % 100);

                // CORRECCIÓN: Si termina en "CERO", lo removemos
                return nombre.Replace(" CERO", "").Trim();
            }

            // Decenas
            if ((n / 10) >= 1)
            {
                if (n < 16)
                {
                    nombre += n switch
                    {
                        10 => "DIEZ",
                        11 => "ONCE",
                        12 => "DOCE",
                        13 => "TRECE",
                        14 => "CATORCE",
                        15 => "QUINCE",
                        _ => ""
                    };
                }
                else if (n < 20) nombre += "DIECI" + ConvertirDecimalALetra(n % 10);
                else if (n == 20) nombre += "VEINTE";
                else if (n < 30) nombre += "VEINTI" + ConvertirDecimalALetra(n % 10);
                else
                {
                    string[] decenas = { "", "", "", "TREINTA", "CUARENTA", "CINCUENTA", "SESENTA", "SETENTA", "OCHENTA", "NOVENTA" };
                    nombre += decenas[(int)(n / 10)];
                    if (n % 10 > 0) nombre += " Y " + ConvertirDecimalALetra(n % 10);
                }
                return nombre.Replace(" CERO", "").Trim();
            }

            // Unidades
            if (n >= 1)
            {
                string[] unidades = { "", "UNO", "DOS", "TRES", "CUATRO", "CINCO", "SEIS", "SIETE", "OCHO", "NUEVE" };
                nombre += unidades[(int)n];
            }

            return nombre.Trim();
        }
    }
}