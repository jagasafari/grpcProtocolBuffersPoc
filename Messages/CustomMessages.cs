using System;

namespace Messages
{
    public class RateMessageRequest
    {
        public string Symbol { get; set; }
        public decimal Ask { get; set; }
        public decimal Bid { get; set; }
        public DateTime TimeStamp { get; set; }
    }

    public class RateResponse
    {
    }

    public class Employee
    {
        public int Id { get; set; }
        public int BadgeNumber { get; set; }
          public string FirstName { get; set; }
          public string LastName { get; set; }
          public float VacationAccrualRate { get; set; }
          public float VacationAccrued { get; set; }
    }

    public class GetAllRequest { }

    public class GetByBadgeNumberRequest
    {
        public int BadgeNumber { get; set; }
    }
    public class EmployeeRequest
    {
        public Employee Employee { get; set; }
    }
    public class EmployeeResponse
    {
        public Employee Employee { get; set; }
    }
    public class AddPhotoRequest
    {
        public byte[] Data { get; set; }
    }
    public class AddPhotoResponse
    {
      public bool IsOk { get; set; }
    }
}
