using System.Globalization;

namespace SaleManagementSys.Helpers
{
    public static class CurrencyHelper
    {
        /// <summary>
        /// Pakistan (PKR) culture with a space between "Rs" and the amount (e.g. "Rs 1,234.56").
        /// </summary>
        public static readonly CultureInfo PkrCulture = CreatePkrCulture();

        private static CultureInfo CreatePkrCulture()
        {
            var culture = (CultureInfo)CultureInfo.GetCultureInfo("en-PK").Clone();
            var nf = (NumberFormatInfo)culture.NumberFormat.Clone();
            nf.CurrencySymbol = "Rs "; // space after Rs
            culture.NumberFormat = nf;
            return culture;
        }
    }
}
