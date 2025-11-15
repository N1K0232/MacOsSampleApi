using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MacOsSampleApi.Shared.Models.Requests;

public record class SavePersonRequest(string FirstName, string LastName);