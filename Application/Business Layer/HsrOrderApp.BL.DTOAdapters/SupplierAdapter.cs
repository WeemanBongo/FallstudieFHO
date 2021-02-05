﻿#region

using System.Collections.Generic;
using System.Linq;
using HsrOrderApp.BL.BusinessComponents;
using HsrOrderApp.BL.DomainModel;
using HsrOrderApp.BL.DTOAdapters.Helper;
using HsrOrderApp.SharedLibraries.DTO;
using HsrOrderApp.SharedLibraries.SharedEnums;

#endregion

namespace HsrOrderApp.BL.DtoAdapters
{
    public class SupplierAdapter
    {
        #region SupplierToDTO

        public static IList<SupplierListDTO> SuppliersToDtos(IQueryable<Supplier> supplier)
        {
            IQueryable<CustomerListDTO> customerDtos = from c in customers
                                                       select new CustomerListDTO()
                                                                  {
                                                                      Id = c.CustomerId,
                                                                      Salutation = c.Salutation,
                                                                      Name = c.Name,
                                                                      FirstName = c.FirstName,
                                                                      NumberOfTotalOrders = GetNumberOfOrdersOfCustomer(c, false),
                                                                      NumberOfOpenOrders = GetNumberOfOrdersOfCustomer(c, true),
                                                                  };
            return customerDtos.ToList();
        }

        public static CustomerDTO CustomerToDto(Customer c)
        {
            CustomerDTO dto = new CustomerDTO()
                                  {
                                      Id = c.CustomerId,
                                      Salutation = c.Salutation,
                                      Name = c.Name,
                                      FirstName = c.FirstName,
                                      Version = c.Version,
                                      Addresses = AddressAdapter.AddressToDtos(c.Addresses)
                                  };

            return dto;
        }

        #region private helpers

        private static int GetNumberOfOrdersOfCustomer(Customer customer, bool draftOnly)
        {
            if (customer.Orders == null)
            {
                return 0;
            }
            if (draftOnly)
            {
                return customer.Orders.Count(o => o.OrderStatus == OrderStatus.Draft);
            }
            return customer.Orders.Count();
        }

        #endregion

        #endregion

        #region DTOToSupplier

        public static Customer DtoToCustomer(CustomerDTO dto)
        {
            Customer customer = new Customer()
                                    {
                                        CustomerId = dto.Id,
                                        Salutation = dto.Salutation,
                                        Name = dto.Name,
                                        FirstName = dto.FirstName,
                                        Version = dto.Version
                                    };
            ValidationHelper.Validate(customer);
            return customer;
        }

        public static IEnumerable<ChangeItem> GetChangeItems(CustomerDTO dto, Customer customer)
        {
            IEnumerable<ChangeItem> changeItems = from c in dto.Changes
                                                  select
                                                      new ChangeItem(c.ChangeType,
                                                                     AddressAdapter.DtoToAddress((AddressDTO) c.Object));
            return changeItems;
        }

        #endregion
    }
}