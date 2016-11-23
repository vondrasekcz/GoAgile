using System.Collections.Generic;
using GoAgile.Models;
using GoAgile.Models.Retrospective;


namespace GoAgile.Dal
{
    public interface IRetrospectiveManager
    {
        /// <summary>
        /// Add Retrospective to database
        /// </summary>
        /// <param name="model">Retrospective model</param>
        /// <param name="user">Creator of retrospective</param>
        /// <Retrospective GuidId</returns>
        string AddModel(CreateRetrospectiveViewModel model, string user);

        /// <summary>
        /// Delete retrospective
        /// </summary>
        /// <param name="guidId">Retrospective GuidId</param>
        /// <returns>True if success, else False</returns>
        bool DeleteModel(string guidId);

        /// <summary>
        /// Find Retrospective
        /// </summary>
        /// <param name="guidId">Retrospective GuidId</param>
        /// <returns>Retrospective model</returns>
        RetrospectiveModel GetModel(string guidId);

        /// <summary>
        /// Find Retrospective
        /// </summary>
        /// <param name="guidId">Retrospective GuidId</param>
        /// <returns>RetrospectiveInitModel for Retrospective page initialization</returns>
        RetrospectiveInitModel GetInitModel(string guidId);

        /// <summary>
        /// Add RetrospectiveItem to database
        /// </summary>
        /// <param name="model">RetrospectiveItemModel</param>
        /// <param name="retrospectiveGuidId">Retrospective GuidId</param>
        /// <returns>RetrospectiveItem GuidId</returns>
        string AddRetrospectiveItem(RetrospectiveItemModel modelItem, string retrospectiveGuidId);

        /// <summary>
        /// Get all shared Retrospective Items
        /// </summary>
        /// <param name="guidId">Retrospective GuidId</param>
        /// <returns>IList of Retrospective shared Items</returns>
        IList<RetrospectiveItemModel> GetAllSharedItems(string guidId);

        /// <summary>
        /// Change Retrospective state to "running"
        /// </summary>
        /// <param name="guidId">Retrospective GuidId</param>
        void ChangeRetrospectiveToRunning(string guidId);

        /// <summary>
        /// Change Retrospective state to "presenting"
        /// </summary>
        /// <param name="guidId">Retrospective GuidId</param>
        void ChangeRetrospectiveToPresenting(string guidId);

        /// <summary>
        /// Change Retrospective state to "voting"
        /// </summary>
        /// <param name="guidId">Retrospective GuidId</param>
        void ChangeToRetrospectiveToVoting(string guidId);

        /// <summary>
        /// Change Retrospective state to "finished"
        /// </summary>
        /// <param name="guidId">Retrospective GuidId</param>
        void ChangeToRetrospectiveToFinished(string guidId);

        /// <summary>
        /// Increase number of votes in RetrospectiveItem
        /// </summary>
        /// <param name="voteModel"></param>
        /// <returns>Votes total</returns>
        int AddVotesToItem(string itemGuid);
    }
}
