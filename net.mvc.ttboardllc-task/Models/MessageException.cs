using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class MessageException : Exception
{
    public int typeException { get; set; }
    public MessageException() : base() { typeException = 3; }
    public MessageException(int TypeException) : base() { typeException = TypeException; }
    public MessageException(string message) : base(message) { typeException = 3; }
    public MessageException(string message, int TypeException) : base(message) { typeException = TypeException; }
    public MessageException(string message, Exception inner) : base(message, inner) { typeException = 3; }
    public MessageException(string message, Exception inner, int TypeException) : base(message, inner) { typeException = TypeException; }
}