using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThirdPartyUtilities.Helpers;

public class Information_error
{
    public int  error_code { get; set; }
    public string error_key { get; set; }
    public string type { get; set; }
    public string error_msg { get; set; }
}


public class Custom_error
{
    public string  error_code { get; set; }
    public string error_key { get; set; }
    public string type { get; set; }
    public string error_msg { get; set; }
}


public class Successful_error
{
    public int error_code { get; set; }
    public string error_key { get; set; }
    public string type { get; set; }
    public string error_msg { get; set; }
}


public class Redirection_error
{
    public int error_code { get; set; }
    public string error_key { get; set; }
    public string type { get; set; }
    public string error_msg { get; set; }
}
public class Client_error
{
    public int error_code { get; set; }
    public string error_key { get; set; }
    public string type { get; set; }
    public string error_msg { get; set; }
}
public class Server_error
{
    public int error_code { get; set; }
    public string error_key { get; set; }
    public string type { get; set; }
    public string error_msg { get; set; }
}


public class ClientError
{
    public int error_code { get; set; }
    public string error_key { get; set; }
    public string type { get; set; }
    public string error_msg { get; set; }
}

public class InformationError
{
    public int error_code { get; set; }
    public string error_key { get; set; }
    public string type { get; set; }
    public string error_msg { get; set; }
}

public class RedirectionError
{
    public int error_code { get; set; }
    public string error_key { get; set; }
    public string type { get; set; }
    public string error_msg { get; set; }
}

public class ErrorCodeDetail
{
    public List<Custom_error> Custom_error { get; set; }
    public List<InformationError> Information_error { get; set; }
    public List<SuccessfulError> Successful_error { get; set; }
    public List<RedirectionError> Redirection_error { get; set; }
    public List<ClientError> Client_error { get; set; }
    public List<ServerError> Server_error { get; set; }
}

public class ServerError
{
    public int error_code { get; set; }
    public string error_key { get; set; }
    public string type { get; set; }
    public string error_msg { get; set; }
}

public class SuccessfulError
{
    public int error_code { get; set; }
    public string error_key { get; set; }
    public string type { get; set; }
    public string error_msg { get; set; }
}
