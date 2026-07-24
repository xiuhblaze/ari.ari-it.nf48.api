using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Exceptions;
using Arysoft.ARI.NF48.Api.Models;
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
            return standardBase == StandardBaseType.ISO9K ? MD5TableType.QMS
                : standardBase == StandardBaseType.ISO14K ? MD5TableType.EMS
                : standardBase == StandardBaseType.ISO22K ? MD5TableType.OHSMS // TODO: De aquí para abajo están en duda
                : standardBase == StandardBaseType.ISO27K ? MD5TableType.QMS
                : standardBase == StandardBaseType.ISO37K ? MD5TableType.EMS
                : standardBase == StandardBaseType.ISO45K ? MD5TableType.OHSMS
                : standardBase == StandardBaseType.HACCP ? MD5TableType.QMS
                : throw new BusinessException("The standard base type is not valid");
        } // GetMD5TableType
    } // AuditCycleCalculations
}