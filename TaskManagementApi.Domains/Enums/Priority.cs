namespace TaskManagementApi.Domains.Enums
{
    public enum Priority
    {
         /// <summary>
         /// The lowest priority. Tasks that are not time-sensitive or can be done when convenient.
         /// </summary>
         Low = 0,
         
         /// <summary>
         /// Standard priority. Important tasks that don't require immediate action.
         /// </summary>
         Medium = 1,
         
         /// <summary>
         /// High priority. Tasks that are important and should be completed soon.
         /// </summary>
         High = 2,
         
         /// <summary>
         /// The highest priority. Tasks that require immediate attention and are critical.
         /// </summary>
         Urgent = 3 
    }
}
