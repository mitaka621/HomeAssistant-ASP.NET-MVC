namespace HomeAssistant.Core.Constants
{
	public static class DataValidationConstants
    {
        public const int NameMinLenght = 3;
        public const int NameMaxLenght = 60;

        public const int QuantityMin = 0;
		public const int QuantityMax = 100;

		public const int DescriptionMinLength = 10;
		public const int DescriptionMaxLength = 500;

		public static string DateTimeFormat = "dd-MM-yyyy HH:mm:ss";
    }
}
