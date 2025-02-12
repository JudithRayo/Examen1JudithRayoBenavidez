// I examen Judith Rayo Benavidez 
// Una fabrica necesita mejorar su sistema de registro de horas trabajadas y calculo de pago para sus empleados. Actualmente, los trabajadores registran su hora de entrada y salida,
// indicando también las actividades realizadas durante la jornada. Dependiendo de la actividad, algunas horas pueden ser consideradas como horas especiales que afectan el cálculo del pago.
//El pago de los empleados se rige por ciertas normas laborales que deben ser consideradas en el sistema. Existen reglas que determinan limites de trabajo semanal, recargos por ciertas condiciones y deducciones que deben aplicarse.

using System;

namespace Fabricacontroldeasistencia
{
    class Program
    {
        const double Tarifa_hora_ordinaria = 1000;
        const double Tarifa_hora_extra = 1500;
        const double Deducción_FCL = 0.01;
        const double Dedución_CCSS = 0.0967;
        const double Bonificación_sin_faltas = 0.10; // Se debe calcular el 10% de salario
        const int Limite_horas_semanales = 48;
        const int Min_horas_diarias = 4;
        const double Limite_retraso = 8.25; // Hora límite de entrada (ejemplo 08:15)

        static void Main(string[] args)
        {
            // Datos de entrada
            String nombre, puesto;
            int identificacion;
            double horasOrdinarias = 0, horasEspeciales = 0;
            double totalHorasSemana = 0;
            double salarioTotal = 0;
            double deducciones = 0;
            double salarioNeto = 0;
            bool tieneFaltas = false;
            bool tieneRetraso = false;

            // Datos del colaborador 
            Console.Write("Ingrese el nombre del colaborador: ");
            nombre = Console.ReadLine();
            Console.Write("Ingrese el puesto del colaborador: ");
            puesto = Console.ReadLine();
            Console.Write("Ingrese la identificación del colaborador: ");
            identificacion = int.Parse(Console.ReadLine());

            // Registro de horas en una semana
            for (int dia = 1; dia <= 7; dia++)
            {
                // Registrar el día de la semana
                Console.WriteLine($"\nRegistro día {dia} de la semana: ");

                // Registrar hora de entrada y salida
                double horasTrabajadas = RegistrarJornadaTrabajo(dia, ref tieneFaltas, ref tieneRetraso);

                if (horasTrabajadas < Min_horas_diarias)
                {
                    Console.WriteLine("Error: El empleado no puede trabajar menos de 4 horas diarias.");
                    return;
                }

                // Para acumular las horas trabajadas en la semana
                totalHorasSemana += horasTrabajadas;

                // Para calcular las horas extras
                if (totalHorasSemana > Limite_horas_semanales)
                {
                    horasEspeciales += horasTrabajadas;
                }
                else
                {
                    horasOrdinarias += horasTrabajadas;
                }
            }

            // Para calcular el salario total
            salarioTotal = horasOrdinarias * Tarifa_hora_ordinaria + horasEspeciales * Tarifa_hora_extra;

            // Si no hubo faltas
            if (!tieneFaltas)
            {
                salarioTotal += salarioTotal * Bonificación_sin_faltas;
            }

            // Para calcular deducciones de ley
            deducciones = salarioTotal * Deducción_FCL + salarioTotal * Dedución_CCSS;

            // Para calcular el salario neto
            salarioNeto = salarioTotal - deducciones;

            // Mostrar el resultado
            MostrarResultados(salarioTotal, deducciones, salarioNeto);
        }

        // Función registro de horas trabajadas por día
        static double RegistrarJornadaTrabajo(int dia, ref bool tieneFaltas, ref bool tieneRetraso)
        {
            double horaEntrada, horaSalida, horasTrabajadas;

            // Registro de hora de entrada
            Console.Write($"Ingrese la hora de entrada del día {dia} (formato 12h, ej. 08:00 AM): ");
            horaEntrada = ConvertirHoraAFormatoDecimal(Console.ReadLine());

            // Registro de hora de salida
            Console.Write($"Ingrese la hora de salida del día {dia} (formato 12h, ej. 05:00 PM): ");
            horaSalida = ConvertirHoraAFormatoDecimal(Console.ReadLine());

            // Validación de hora de entrada y salida
            if (horaSalida < horaEntrada)
            {
                Console.WriteLine("ERROR: La hora de salida no puede ser anterior a la hora de entrada.");
                return 0;
            }

            // Calcular horas trabajadas en el día
            horasTrabajadas = horaSalida - horaEntrada;

            // Validar que no se trabajen menos de 4 horas en un día
            if (horasTrabajadas < Min_horas_diarias)
            {
                tieneFaltas = true;
                Console.WriteLine("ERROR: El colaborador no ha trabajado las horas mínimas requeridas.");
            }

            // Penalización por retrasos (si la hora de entrada es más tarde de lo permitido)
            if (horaEntrada > Limite_retraso) // Si entra después de las 08:15 AM
            {
                tieneRetraso = true;
                double penalizacion = 0.05 * horasTrabajadas; // Penalización de 5% de las horas trabajadas
                horasTrabajadas -= penalizacion;
                Console.WriteLine("El colaborador ha sido penalizado por retraso.");
            }

            return horasTrabajadas;
        }

        // Función para convertir la hora de formato "hh:mm AM/PM" a formato decimal
        static double ConvertirHoraAFormatoDecimal(string hora)
        {
            try
            {
                string[] partes = hora.Split(' '); // Parte antes del espacio es la hora, después es AM/PM
                string[] horaParte = partes[0].Split(':'); // Parte antes de los dos puntos es la hora, después es minutos

                int horas = int.Parse(horaParte[0]);
                int minutos = int.Parse(horaParte[1]);

                // Si es PM y no es 12 PM, sumamos 12 horas
                if (partes[1].ToUpper() == "PM" && horas != 12)
                {
                    horas += 12;
                }
                // Si es AM y es 12 AM, cambiamos la hora a 0 (medianoche)
                if (partes[1].ToUpper() == "AM" && horas == 12)
                {
                    horas = 0;
                }

                double horaDecimal = horas + (minutos / 60.0);
                return horaDecimal;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: El formato de la hora ingresada es incorrecto. {ex.Message}");
                return 0; // Devolver 0 en caso de error
            }
        }

        // Función para mostrar los resultados
        static void MostrarResultados(double salarioTotal, double deducciones, double salarioNeto)
        {
            Console.WriteLine("\n------ Resultados ------");
            Console.WriteLine($"Salario Total: {salarioTotal:C}");
            Console.WriteLine($"Deducciones: {deducciones:C}");
            Console.WriteLine($"Salario Neto: {salarioNeto:C}");
            Console.ReadKey();
        }
    }
}