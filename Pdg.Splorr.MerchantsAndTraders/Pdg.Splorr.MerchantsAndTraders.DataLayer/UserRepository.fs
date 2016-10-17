namespace Pdg.Splorr.MerchantsAndTraders.DataLayer

module UserRepository =
    let exists (userId:string) (context:MaToSplorrProvider.dataContext) : bool =
        query{
            for user in context.Dbo.AspNetUsers do
            where (user.Id=userId)
            select(user)
        }
        |> Seq.exists(fun x->true)

