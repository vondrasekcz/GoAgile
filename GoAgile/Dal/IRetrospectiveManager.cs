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
        void AddRetrospectiveItem(RetrospectiveItemModel modelItem, string retrospectiveGuidId);

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
        /// <param name="moderator">Owner name to check</param>
        /// <returns>False if moderator isn't owner</returns>
        bool ChangeRetrospectiveToRunning(string guidId, string moderator);

        /// <summary>
        /// Change Retrospective state to "presenting"
        /// </summary>
        /// <param name="guidId">Retrospective GuidId</param>
        /// <param name="moderator">Owner name to check</param>
        /// <returns>False if moderator isn't owner</returns>
        bool ChangeRetrospectiveToPresenting(string guidId, string moderator);

        /// <summary>
        /// Change Retrospective state to "voting"
        /// </summary>
        /// <param name="guidId">Retrospective GuidId</param>
        /// <param name="moderator">Owner name to check</param>
        /// <returns>False if moderator isn't owner</returns>
        bool ChangeToRetrospectiveToVoting(string guidId, string moderator);

        /// <summary>
        /// Change Retrospective state to "finished"
        /// </summary>
        /// <param name="guidId">Retrospective GuidId</param>
        /// <param name="moderator">Owner name to check</param>
        /// <returns>False if moderator isn't owner</returns>
        bool ChangeToRetrospectiveToFinished(string guidId, string moderator);

        /// <summary>
        /// Get Retospective phase
        /// </summary>
        /// <param name="guidId"></param>
        /// <returns></returns>
        string GetRetrospectivePhase(string guidId);

        /// <summary>
        /// Determine if retrospectiveItem exist
        /// </summary>
        /// <param name="guidId"></param>
        /// <returns></returns>
        bool ExistRetrospectiveItem(string itemGuidId, string eventGuidId);

        /// <summary>
        /// Increase number of votes in RetrospectiveItem
        /// </summary>
        /// <param name="voteModel"></param>
        /// <returns>Votes total</returns>
        int AddVotesToItem(string itemGuid);

        /// <summary>
        /// Determinate if voting is enables, and return max votes
        /// </summary>
        /// <param name="guidId"></param>
        /// <returns>0 - disabled voding, less than 0 - invalid phase, more than 0 - max votes</returns>
        int GetMaxVotes(string guidId);

        /// <summary>
        /// Validate owner
        /// </summary>
        /// <param name="guidId"></param>
        /// <param name="name">moderator name</param>
        /// <returns></returns>
        bool ValidateOwner(string guidId, string name);

        /// <summary>
        /// Exist this retrospective
        /// </summary>
        /// <param name="guidId"></param>
        /// <returns></returns>
        bool ExistRetrospective(string guidId);
    }
}
