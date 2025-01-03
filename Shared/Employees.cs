﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCMS_wasm.Shared
{
    public class Employee : BaseModel
    {
        public string? IDNumber { get; set; }
        public string? Auth0_Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? SSS { get; set; }
        public string? PagIbig { get; set; }
        public string? PHIC { get; set; }
        public string Profile { get; set; } = "images/user-alt-solid.svg";

        // Additional fields
        public DateTime? DateOfBirth { get; set; }
        public string? PlaceOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? CivilStatus { get; set; }             
        public string? Email { get; set; }

        public string? Position { get; set; }
        public Department Department { get; set; } = Department.None;
        public EmploymentStatus EmploymentStatus { get; set; } = EmploymentStatus.Regular; 
        public DateTime? DateHired { get; set; }
        public DateTime? DateResigned { get; set; } = null;
        public decimal? Salary { get; set; } = null;        
        
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactRelationship { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public string? EmergencyContactAddress { get; set; }

        public string? BeneficiaryName { get; set; }
        public string? BeneficiaryRelationship { get; set; }
        public string? BeneficiaryContactInfo { get; set; }           

        public string? imageUrl { get; set; }

        public string? CardReference { get; set; }
    }

    public enum Department
    {
        None = 0,
        HR = 1,
        Gas = 2,
        Hotel = 3
    }

    public enum EmploymentStatus
    {
        Resigned = 0,
        Regular = 1,
        Probationary = 2
    }

}
