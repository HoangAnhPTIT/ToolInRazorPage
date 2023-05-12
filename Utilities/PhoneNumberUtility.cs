namespace SampleApp.Utilities
{
    public static class PhoneNumberUtility
    {
        public static string StandardizedPhoneNumber(object text)
        {
            var stringValue = text.ToString().Trim();
            if (stringValue.StartsWith("84"))
            {
                stringValue = "0" + stringValue.Substring(2);
            }
            if (stringValue.StartsWith("+84"))
            {
                stringValue = "0" + stringValue.Substring(3);
            }
            if (!stringValue.StartsWith("0"))
            {
                stringValue = "0" + stringValue;
            }

            return stringValue;
        }
    }
}
