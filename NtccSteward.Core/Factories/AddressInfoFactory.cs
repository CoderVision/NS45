using NtccSteward.Core.Interfaces.Common.Address;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtccSteward.Core.Factories
{
    public class AddressInfoFactory
    {
        public T CreateAddress<T>(IAddress source)
            where T : IAddress, new()
        {
            var target = CreateAddressInfo<T>(source);

            target.Line1 = source.Line1;
            target.Line2 = source.Line2;
            target.Line3 = source.Line3;
            target.City = source.City;
            target.State = source.State;
            target.Zip = source.Zip;

            return target;
        }

        public T CreatePhone<T>(IPhone source)
                where T : IPhone, new()
        {
            var target = CreateAddressInfo<T>(source);

            target.PhoneNumber = source.PhoneNumber;
            target.PhoneType = source.PhoneType;

            return target;
        }

        public T CreateEmail<T>(IEmail source)
            where T : IEmail, new()
        {
            var target = CreateAddressInfo<T>(source);

            target.EmailAddress = source.EmailAddress;

            return target;
        }


        private T CreateAddressInfo<T>(IAddressInfo addyinfo)
            where T : IAddressInfo, new()
        {
            var addy = new T();
            addy.ContactInfoId = addyinfo.ContactInfoId;
            addy.IdentityId = addyinfo.IdentityId;
            addy.ContactInfoType = addyinfo.ContactInfoType;
            addy.ContactInfoLocationType = addyinfo.ContactInfoLocationType;
            addy.ModifiedByIdentityId = addyinfo.ModifiedByIdentityId;
            addy.Preferred = addyinfo.Preferred;
            addy.Verified = addyinfo.Verified;
            addy.Archived = addyinfo.Archived;

            return addy;
        }
    }
}
