namespace Pdg.Splorr.MerchantsAndTraders.DataLayer

open FSharp.Data.Sql
open System

module Constants =
    let [<Literal>] connectionString="Initial Catalog=MerchantsAndTraders;Data Source=(local);Integrated Security=True"

type MaToSplorrProvider = 
    SqlDataProvider<
        Common.DatabaseProviderTypes.MSSQLSERVER,
        Constants.connectionString,
        UseOptionTypes = true>

module Context =
    let create () =
        MaToSplorrProvider.GetDataContext();
