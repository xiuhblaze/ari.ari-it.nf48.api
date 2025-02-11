namespace Arysoft.ARI.NF48.Api.Tools
{
    public partial class Strings
    {
        public static string FullName(string firstName, string middleName, string lastName)
        {
            var fullName = firstName;

            fullName += !string.IsNullOrEmpty(middleName) ? " " + middleName : string.Empty;
            fullName += !string.IsNullOrEmpty(lastName) ? " " + lastName : string.Empty;

            return fullName;
        } // FullName
    }
}