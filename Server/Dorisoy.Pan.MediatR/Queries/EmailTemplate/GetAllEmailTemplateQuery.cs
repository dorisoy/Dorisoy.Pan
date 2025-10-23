using MediatR;
using System.Collections.Generic;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Helper;

namespace Dorisoy.Pan.MediatR.Queries
{
    public class GetAllEmailTemplateQuery : IRequest<ServiceResponse<List<EmailTemplateDto>>>
    {

    }
}
