
using ImDal;
using ImModel;

namespace ImBLL
{
    public class FeedbackBll
    {
        readonly FeedbackDal _feedbackDal=new FeedbackDal();
        public bool AddFeedback(Feedback model)
        {
            if (string.IsNullOrEmpty(model.UserIdentity)||string.IsNullOrEmpty(model.Description))
            {
                return false;
            }
            model.FeedBackState = FeedBackStateEnum.BeSolve;
            return _feedbackDal.AddFeedback(model);
        }
    }
}
