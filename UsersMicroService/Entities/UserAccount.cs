using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UsersMicroService.Entities
{
	public class UserAccount
	{
		public UserAccount()
		{
			CreatedOn = DateTime.UtcNow;
			UpdatedOn = DateTime.UtcNow;
		}

		[Key]
		public int UserId { get; set; }
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public string? MobileNumber { get; set; }
		public string? ProfileName { get; set; }
		public string AuthNetProfileId { get; set; }
		public string? InvestingLocationCityCounty1 { get; set; }
		public string? InvestingLocationState1 { get; set; }
		public string? InvestingLocationCityCounty2 { get; set; }
		public string? InvestingLocationState2 { get; set; }
		public string? InvestingLocationCityCounty3 { get; set; }
		public string? InvestingLocationState3 { get; set; }
		public string? WhatDoYouDo { get; set; }
		public string? ContactMethod1 { get; set; }
		public string? ContactMethod2 { get; set; }
		public string? BillingAddress1 { get; set; }
		public string? BillingAddress2 { get; set; }
		public string? BillingCity { get; set; }
		public string? BillingState { get; set; }
		public string? BillingZip { get; set; }
		public string? ReturnName { get; set; }
		public string? ReturnAddress1 { get; set; }
		public string? ReturnAddress2 { get; set; }
		public string? ReturnCity { get; set; }
		public string? ReturnState { get; set; }
		public string? ReturnZip { get; set; }
		public string? ReturnCounty { get; set; }
		public bool? PushVacant { get; set; }
		public bool? PushPreforeclosure { get; set; }
		public bool? PushREO { get; set; }
		public decimal? AccountBalance { get; set; }
		public int CreatedUserId { get; set; }
		public DateTime CreatedOn { get; set; }
		public int UpdatedUserId { get; set; }
		public DateTime UpdatedOn { get; set; }

	}
}