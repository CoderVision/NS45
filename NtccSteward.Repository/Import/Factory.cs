using NtccSteward.Core.Models.Church;
using NtccSteward.Core.Models.Import;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NtccSteward.Repository.Import
{
    public class Factory
    {
        public Church CreateChurch(ChurchInfo churchInfo)
        {
            var church = new Church();
            church.Name = churchInfo.City1;
            church.Address = churchInfo.Address;
            church.City = churchInfo.City1;
            church.State = churchInfo.State;
            church.Zip = ParseNumber(churchInfo.Zip1);
            church.Phone = ParseNumber(churchInfo.Phone);
            return church;
        }

        public string ParseNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return "";

            var ph = "";
            foreach (var c in phoneNumber)
            {
                if (Char.IsNumber(c))
                    ph += c;
            }

            return ph;
        }
    }
}