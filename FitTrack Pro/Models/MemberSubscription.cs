namespace FitTrack_Pro.Models
{
	public class MemberSubscription : BaseEntity
	{
		public int MemberId { get; set; }
		public  Member? Member { get; set; }

		public int SubscriptionPlanId { get; set; }
		public  SubscriptionPlan? SubscriptionPlan { get; set; }

		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public decimal PaidAmount { get; set; }
		public bool IsActive { get; set; }
	}
}
