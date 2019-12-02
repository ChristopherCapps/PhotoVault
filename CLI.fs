module CLI

open Argu

type CreateArgs =
    | Name of name: string
with 
    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | Name _ -> "The friendly name of the vault to create"
// and StatusArgs =
//     | Status

// and UpdateArgs =
//     | Update

and VaultArgs =
    | Vault_Directory of path: string option
    | [<CliPrefix(CliPrefix.None)>] Create of ParseResults<CreateArgs>
    | [<CliPrefix(CliPrefix.None)>] Status
    | [<CliPrefix(CliPrefix.None)>] Update
with 
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Vault_Directory _ -> "specifies the parent directory of the vault on which the command applies"
            | Create _ -> "Creates a new vault"
            | Status _ -> "Displays the status of the vault"
            | Update _ -> "Forces a rescan of the vault, applying naming rules and quarantining duplicates"

let parser = ArgumentParser.Create<VaultArgs>(programName = "vault")            