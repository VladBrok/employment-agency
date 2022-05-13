﻿using System;
using System.Collections.Generic;

namespace Temp
{
    public partial class Seeker
    {
        public Seeker()
        {
            Applications = new HashSet<Application>();
        }

        public int Id { get; set; }
        public int StatusId { get; set; }
        public int AddressId { get; set; }
        public int SpecialityId { get; set; }
        public string LastName { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string? Patronymic { get; set; }
        public string? Phone { get; set; }
        public DateOnly Birthday { get; set; }
        public string RegistrationCity { get; set; } = null!;
        public bool Recommended { get; set; }
        public bool Pol { get; set; }
        public string? Education { get; set; }

        public virtual Address Address { get; set; } = null!;
        public virtual Position Speciality { get; set; } = null!;
        public virtual Status Status { get; set; } = null!;
        public virtual ICollection<Application> Applications { get; set; }
    }
}
