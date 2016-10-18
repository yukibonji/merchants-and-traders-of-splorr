namespace Pdg.Splorr.MerchantsAndTraders.Web.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Web
open System.Web.Mvc
open System.Web.Routing
open System.Web.Mvc.Ajax
open Pdg.Splorr.MerchantsAndTraders.BusinessLayer
open Pdg.Splorr.MerchantsAndTraders.DataLayer
open Microsoft.AspNet.Identity
open Pdg.Splorr.MerchantsAndTraders.Web.Models

[<Authorize>]
type AgentController() =
    inherit Controller()

    member this.Index () : ActionResult = 

        let agentListResult =
            (this.User.Identity.GetUserId())
            |> AgentService.retrieveList 

        match agentListResult with
        | ServiceResult.Success agentList ->
            this.View(agentList) :> ActionResult
        | _ ->
            //TODO: put error list where the user can see it
            this.View("Error") :> ActionResult
    
    [<HttpGet>]
    member this.Add () : ActionResult =
        let availableWorldList =
            match WorldService.retrieveAvailableList (this.User.Identity.GetUserId()) with
            | ServiceResult.Success worldList -> 
                worldList
                |> Seq.map (fun world -> SelectListItem(Text=world.WorldName,Value=world.WorldId.ToString()))
            | _ -> Seq.empty
        this.ViewData.Add("AvailableWorldList", availableWorldList)
        this.View(NewAgentModel()) :> ActionResult

    [<HttpPost>]
    [<ValidateAntiForgeryToken>]
    member this.Add(model:NewAgentModel) : ActionResult =
        if this.ModelState.IsValid |> not then
            this.View(model) :> ActionResult
        else
            match {Agent.AgentName = model.AgentName; WorldId = model.WorldId; AgentId=0; UserId = this.User.Identity.GetUserId()} |> AgentService.create with
            | ServiceResult.Success agent ->
                let rvd = new RouteValueDictionary()
                rvd.Add("id",agent.AgentId)
                this.RedirectToAction("Detail","Agent", rvd) :> ActionResult
            | ServiceResult.Failure messages ->
                messages
                |> Seq.iter(fun message -> this.ModelState.AddModelError("",message))
                this.View(model) :> ActionResult

    [<HttpGet>]
    member this.Remove (id:int) : ActionResult =
        //TODO: check that A) id exists and B) id "belongs" to the current user(it does get done in the service layer)
        this.View(ConfirmDeleteModel(Id=id)) :> ActionResult

    [<HttpPost>]
    [<ValidateAntiForgeryToken>]
    member this.Remove(model:ConfirmDeleteModel) : ActionResult =
        if this.ModelState.IsValid |> not then
            this.View(model) :> ActionResult
        else
            if model.Confirm |> not then
                this.RedirectToAction("Index") :> ActionResult
            else
                match (this.User.Identity.GetUserId(), model.Id) ||> AgentService.delete with
                | ServiceResult.Success agent ->
                    this.RedirectToAction("Index","Agent") :> ActionResult
                | ServiceResult.Failure messages ->
                    messages
                    |> Seq.iter(fun message -> this.ModelState.AddModelError("",message))
                    this.View(model) :> ActionResult

    [<HttpGet>]
    member this.Detail (id:int) : ActionResult =
        match (this.User.Identity.GetUserId(), id) ||> AgentService.retrieve with
        | ServiceResult.Success agent ->
            this.View(agent) :> ActionResult
        | ServiceResult.Failure messages ->
            this.RedirectToAction("Index") :> ActionResult

    [<HttpGet>]
    member this.Edit (id:int) : ActionResult =
        match (this.User.Identity.GetUserId(), id) ||> AgentService.retrieve with
        | ServiceResult.Success agent ->
            this.View(EditAgentModel(AgentId=agent.AgentId,AgentName=agent.AgentName)) :> ActionResult
        | ServiceResult.Failure messages ->
            this.RedirectToAction("Index") :> ActionResult

    [<HttpPost>]
    [<ValidateAntiForgeryToken>]
    member this.Edit(model:EditAgentModel) : ActionResult =
        let agent = {Agent.AgentId = model.AgentId; AgentName=model.AgentName; UserId = this.User.Identity.GetUserId(); WorldId=0}
        match AgentService.update agent with
        | ServiceResult.Success agent ->
            this.RedirectToAction("Index") :> ActionResult
        | ServiceResult.Failure messages ->
            messages
            |> Seq.iter(fun message -> this.ModelState.AddModelError("",message))
            this.View(model) :> ActionResult
        


