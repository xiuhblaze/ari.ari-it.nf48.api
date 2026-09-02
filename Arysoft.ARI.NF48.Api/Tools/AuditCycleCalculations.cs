using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Exceptions;
using Arysoft.ARI.NF48.Api.Models;
using System.Collections.Generic;
using System.Linq;

namespace Arysoft.ARI.NF48.Api.Tools
{
    internal static class AuditCycleCalculations
    {
        /// <summary>
        /// Obtiene el numero de días según el nivel de riesgo proporcionado
        /// dentro de la tabla MD5.
        /// </summary>
        /// <param name="md5">Objeto con el nivel de empleados MD5</param>
        /// <param name="riskLevel"></param>
        /// <returns></returns>
        public static decimal GetInitialAuditDaysByRiskLevelCategory(MD5 md5, RiskLevelCategoryType riskLevel)
        {
            return riskLevel == RiskLevelCategoryType.High ? md5.HighDays ?? 0
                : riskLevel == RiskLevelCategoryType.Medium ? md5.Days ?? 0
                : riskLevel == RiskLevelCategoryType.Low ? md5.LowDays ?? 0
                : riskLevel == RiskLevelCategoryType.Limited ? md5.LimDays ?? 0
                : md5.Days ?? 0;
        } // GetDaysByRiskLevelCategory

        /// <summary>
        /// Obtiene la suma adicional de dias iniciales según la categoría
        /// ISO 22000 y el número de planes HACCP.
        /// </summary>
        /// <param name="days"></param>
        /// <param name="category22K"></param>
        /// <param name="haccpCount"></param>
        /// <returns></returns>
        public static decimal GetInitialAuditDaysForISO22K(
            decimal days, 
            Category22K category22K,
            int haccpCount
        )
        {   
            days += category22K?.BasicDaysTD ?? 0;
            if (haccpCount > 1)
            {
                int additionalHACCPDays = haccpCount - 1;
                days += additionalHACCPDays * (category22K?.HACCPDaysTH ?? 0);
            }
            return days;
        } // GetInitialAuditDaysForISO22K

        /// <summary>
        /// Calcula el nivel de riesgo máximo del AppForm recibido.
        /// Dejar por defecto Medium si no hay niveles de riesgo o 
        /// si todos son Nothing. Considerando estandares que no tienen 
        /// niveles de riesgo, como ISO 9001, ISO 14001, ISO 45001, etc.
        /// </summary>
        /// <param name="appForm"></param>
        /// <returns></returns>
        public static RiskLevelCategoryType GetMaxRiskLevelCategory(AppForm appForm)
        {
            if (appForm == null || appForm.RiskLevels == null || !appForm.RiskLevels.Any())
            {
                return RiskLevelCategoryType.Medium;
            }

            var maxRiskLevel = appForm.RiskLevels
                .Where(rl => rl.Category != RiskLevelCategoryType.Nothing)
                .OrderByDescending(rl => rl.Category)
                .FirstOrDefault();

            return maxRiskLevel?.Category ?? RiskLevelCategoryType.Medium;
        } // GetMaximumRiskLevelCategory

        public static MD5TableType GetMD5TableType(StandardBaseType standardBase)
        {
            return standardBase == StandardBaseType.ISO9K ? MD5TableType.QMS    // Ok
                : standardBase == StandardBaseType.ISO14K ? MD5TableType.EMS    // Ok
                : standardBase == StandardBaseType.ISO22K ? MD5TableType.FTE    // Este tiene su propia lista, no es MD5, pero se puede quedar aquí
                : standardBase == StandardBaseType.ISO27K ? MD5TableType.QMS    // En duda
                : standardBase == StandardBaseType.ISO37K ? MD5TableType.EMS    // En duda
                : standardBase == StandardBaseType.ISO45K ? MD5TableType.OHSMS  // Ok
                : standardBase == StandardBaseType.HACCP ? MD5TableType.QMS     // En duda
                : throw new BusinessException("The standard base type is not valid");
        } // GetMD5TableType

        /// <summary>
        /// Obtiene la lista de pasos de auditoría que corresponden a un ciclo de auditoría, 
        /// considerando el tipo de ciclo, el paso inicial y la periodicidad.
        /// </summary>
        /// <param name="cycleType">Tipo de ciclo, ya sea Inicial, de Recertificación o de Transferencia</param>
        /// <param name="initialStep">Paso inicial del ciclo - para ciclos de Transferencia</param>
        /// <param name="periodicity">Periodicidad del ciclo</param>
        /// <returns>Lista de pasos de auditoría correspondientes</returns>
        public static List<AuditStepType> GetStepList(AuditCycleType cycleType, AuditStepType initialStep, AuditCyclePeriodicityType periodicity)
        {
            var stepList = new List<AuditStepType>();

            switch (cycleType)
            {
                case AuditCycleType.Initial:
                    stepList.Add(AuditStepType.Stage1); // para registrar los días de ST1
                    stepList.Add(AuditStepType.Stage2);
                    stepList.Add(AuditStepType.Surveillance1);
                    stepList.Add(AuditStepType.Surveillance2);
                    if (periodicity == AuditCyclePeriodicityType.Biannual)
                    {
                        stepList.Add(AuditStepType.Surveillance3);
                        stepList.Add(AuditStepType.Surveillance4);
                        stepList.Add(AuditStepType.Surveillance5);
                    }
                    break;
                case AuditCycleType.Recertification:
                    stepList.Add(AuditStepType.Recertification);
                    stepList.Add(AuditStepType.Surveillance1);
                    stepList.Add(AuditStepType.Surveillance2);
                    if (periodicity == AuditCyclePeriodicityType.Biannual)
                    {
                        stepList.Add(AuditStepType.Surveillance3);
                        stepList.Add(AuditStepType.Surveillance4);
                        stepList.Add(AuditStepType.Surveillance5);
                    }
                    break;
                case AuditCycleType.Transfer:
                    switch (initialStep)
                    {
                        case AuditStepType.Recertification:
                            stepList.Add(AuditStepType.Recertification);
                            stepList.Add(AuditStepType.Surveillance1);
                            stepList.Add(AuditStepType.Surveillance2);
                            if (periodicity == AuditCyclePeriodicityType.Biannual)
                            {
                                stepList.Add(AuditStepType.Surveillance3);
                                stepList.Add(AuditStepType.Surveillance4);
                                stepList.Add(AuditStepType.Surveillance5);
                            }
                            break;
                        case AuditStepType.Surveillance1:
                            stepList.Add(AuditStepType.Surveillance1);
                            stepList.Add(AuditStepType.Surveillance2);
                            if (periodicity == AuditCyclePeriodicityType.Biannual)
                            {
                                stepList.Add(AuditStepType.Surveillance3);
                                stepList.Add(AuditStepType.Surveillance4);
                                stepList.Add(AuditStepType.Surveillance5);
                            }
                            break;
                        case AuditStepType.Surveillance2:
                            stepList.Add(AuditStepType.Surveillance2);
                            if (periodicity == AuditCyclePeriodicityType.Biannual)
                            {
                                stepList.Add(AuditStepType.Surveillance3);
                                stepList.Add(AuditStepType.Surveillance4);
                                stepList.Add(AuditStepType.Surveillance5);
                            }
                            break;
                    }
                    break;
            }

            return stepList;
        } // GetStepList
    } // AuditCycleCalculations
}