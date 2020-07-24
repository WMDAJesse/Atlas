variable "AZURE_CLIENT_ID" {
  type        = string
  description = "Client ID used for authenticating to manage Azure resources from code."
}

variable "AZURE_CLIENT_SECRET" {
  type        = string
  description = "Client secret used for authenticating to manage Azure resources from code."
}

variable "AZURE_SUBSCRIPTION_ID" {
  type        = string
  description = "ID of the Azure subscription into which the system will be deployed."
}

variable "ENVIRONMENT" {
  type        = string
  description = "Prepended to all ATLAS resources, to indicate which environment of the installation they represent. Some alphanumeric characters must be present, as non-alphanumeric characters will be stripped from the storage account name. Max 8 alphanumeric characters. e.g. DEV/UAT/LIVE"
}

variable "LOCATION" {
  type        = string
  default     = "uksouth"
  description = "GeoLocation of all Azure resources for this ATLAS installation."
}

variable "TERRAFORM_KEY" {
  type        = string
  default     = "atlas.terraform.tfstate"
  description = "Key used to identify the ATLAS system within a terraform backend. Used as a remote state to connect webhooks appropriately."
}

variable "TERRAFORM_WEBHOOKS_KEY" {
  type        = string
  default     = "atlas.terraform.tfstate"
  description = "Key used to identify the Webhooks of the ATLAS system within a terraform backend. Registered separately as webhook can only be configured once endpoints have been deployed to Azure."
}

variable "TERRAFORM_RESOURCE_GROUP_NAME" {
  type        = string
  description = "Resource group in which the terraform backend is deployed."
}

variable "TERRAFORM_STORAGE_ACCOUNT_NAME" {
  type        = string
  description = "Name of the storage account in which the terraform backend is deployed."
}

variable "TERRAFORM_STORAGE_CONTAINER_NAME" {
  type        = string
  default     = "terraform-state"
  description = "Name of the container within the storage account in which the terraform backend is deployed."
}