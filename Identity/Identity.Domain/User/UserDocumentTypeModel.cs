using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.User;

public class UserDocumentTypeModel
{
    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// DocumentType
    /// </summary>
    public string DocumentType { get; set; }



    /// <summary>
    /// IsMandatory
    /// </summary>
    public bool  IsMandatory { get; set; }


    /// <summary>
    /// ShortDescription
    /// </summary>
    public string ShortDescription { get; set; }    

}
