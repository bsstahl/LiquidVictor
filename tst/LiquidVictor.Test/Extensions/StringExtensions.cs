using System;
using System.Collections.Generic;
using System.Text;

namespace LiquidVictor.Test.Extensions
{
    public static class StringExtensions
    {
        const string _tinyBase64EncodedPng = "iVBORw0KGgoAAAANSUhEUgAAADUAAAAqCAIAAACcBIRpAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjEuNWRHWFIAAANOSURBVFhHtZitWzJBFMU3EAwGAoFAMBANBgPBQCASjATDGwgGg5Hg8xgIBgKBQCAQCEYCgWAkGAj8AQYDwWAwGAwE3sPsmX2WO7vDfsz+irJn7+UwZ3d2Zr3tdjscDh8eHu7v7weDwcfHxz4BqOr1ev8UqF0ulxTy8f7+/vj4iJ7dbvfl5QXf4uE/L0Sr1eK5Vp6fn1mgKJVKn5+f1LKy2+0qlQo7Kvr9vgez/KRoNps83cpisWCBBr2oZeXt7Y29NKvVKqO/v7+/crnMGsXV1RW1rIgka7UaDmb0B0Q7sNlsqKXHDBcXIo5n92fG8fT0RC09Zjf/12b3BxAByxT1ep1CekQaQatc/hAByzTZIo68c30pl7/1es0yTbaIzXCDaTiXP4AgWKnwb7q0iHCvr68p5PeHAWOlBpMWtWSY4eJ5Ri2/PwTBSo0/LyRHhItHER5r1PL7A5iZWaxIG7EIVzxgHfhDHCzWJI/YDHcymVBTOPB3WGUcgxUNtVOIcM/Ozn5+fqgpHPgDqGK9AkOCgaFmRYR7e3tLQePGH0JhvQYDQy0eM9zX11dqGjf+vr+/EQ1bKDAw1OIR4Z6fn2NZRE3jxh9ANGyhSBKxCPfu7o5CCGf+ZrMZW2jsi34z3MhLwpk/RIOA2EUROR4BIty48XbmD8AQuygir6cAEW7clOTSn7kpmc/n1I4xw42b0l36M781LmIR7sXFBQUDl/4ANtFspIiLWIRrWTU69nfYER6D+5qaxhxmy6rbsT8gNiXmI0uEa9+YuvfX6/XYS4Hnyu/vLzWFCNe+sXfvD2GxlyYcsRmu/cWIe3/g8vKS7RThiEW4Nzc3FGIoxN/hvU4IRIwFhC+JcEejkX88jkL8mZsSf1UswsVWI/AdRyH+AIJjR4W/qxDhJnmXV5Q/sSnxh0qEa06NJkX5+/r6gic2VYzH43C45rwTSVH+AOJjU4WYVjqdDs+zIv1Vq1UcSQiem7jk2clgOp2yaRRxSxuB9JcWyyIZ8YlNSUC5XLYsDcPk9YdBYqcoxKYkADcKzziFJ97Ep8XcEYZBiDzvGEw0POMUHm57XMjiXksIliq4T9kpClydjUaDZ2va7TblBHj8WyTb7RaLAB/775Hs9/8Box6kXQx0V9UAAAAASUVORK5CYII=";
        
        public static string TinyBase64EncodedPng(this string ignore)
        {
            return _tinyBase64EncodedPng;
        }
    }
}
