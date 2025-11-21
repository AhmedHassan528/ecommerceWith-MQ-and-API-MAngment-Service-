namespace MultiTenancy.Dtos.PaymobDtos
{
    public class AuthRequest
    {
        public string api_key { get; set; }
    }

    public class AuthResponse
    {
        public string token { get; set; }
        public Profile profile { get; set; }
    }

    public class Profile
    {
        public int id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phone_number { get; set; }
        public string created_at { get; set; }
    }

    public class OrderRequest
    {
        public string auth_token { get; set; }
        public bool delivery_needed { get; set; }
        public decimal amount_cents { get; set; }
        public string currency { get; set; }
        public List<object> items { get; set; }
    }

    public class OrderResponse
    {
        public int id { get; set; }
        public string created_at { get; set; }
        public bool delivery_needed { get; set; }
        public Merchant merchant { get; set; }
        public decimal amount_cents { get; set; }
        public string currency { get; set; }
        public List<object> items { get; set; }
    }

    public class Merchant
    {
        public string id { get; set; }
        public string name { get; set; }
        public string created_at { get; set; }
    }

    public class PaymentKeyRequest
    {
        public string auth_token { get; set; }
        public decimal amount_cents { get; set; }
        public int expiration { get; set; }
        public int order_id { get; set; }
        public BillingData billing_data { get; set; }
        public string currency { get; set; }
        public int integration_id { get; set; }
    }

    public class BillingData
    {
        public string email { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string phone_number { get; set; }
        public string apartment { get; set; }
        public string floor { get; set; }
        public string street { get; set; }
        public string building { get; set; }
        public string shipping_method { get; set; }
        public string postal_code { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string state { get; set; }
    }

    public class PaymentKeyResponse
    {
        public string token { get; set; }
    }

}
