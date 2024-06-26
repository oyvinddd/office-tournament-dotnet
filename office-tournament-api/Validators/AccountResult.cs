﻿using office_tournament_api.office_tournament_db;

namespace office_tournament_api.Validators
{
    public class AccountResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; }
        public Account? Account { get; set; }
        public string Token { get; set; }

        public AccountResult() 
        {
        }

        public AccountResult(bool isValid, List<string> errors)
        {
            IsValid = isValid;
            Errors = errors;
        }
    }
}
