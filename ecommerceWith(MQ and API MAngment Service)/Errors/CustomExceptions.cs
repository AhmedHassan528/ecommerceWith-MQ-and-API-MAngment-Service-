namespace ecommerceWith_MQ_and_API_MAngment_Service_.Errors
{
    public class CustomExceptions
    {
        public class NotFoundException : Exception
        {
            public NotFoundException(string message) : base(message) { }
        }

        public class BadRequestException : Exception
        {
            public BadRequestException(string message) : base(message) { }
        }
        public class UnauthorizedException : Exception
        {
            public UnauthorizedException(string message) : base(message) { }
        }

    }
}
