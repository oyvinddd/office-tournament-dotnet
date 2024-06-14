namespace office_tournament_api.DTOs
{
    public class DTOAccountInfoResponse
    {
        public DTOAccountResponse Account { get; set; }
        public string Token { get; set; }
        public DTOAccountInfoResponse() { }

        public DTOAccountInfoResponse(DTOAccountResponse dtoAccountResponse, string token)
        {
            Account = dtoAccountResponse;
            Token = token;
        }
    }
}
