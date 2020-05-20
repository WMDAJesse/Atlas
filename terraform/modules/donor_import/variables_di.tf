// Variables that allow for dependency inversion of other terraformed resources.

variable "app_service_plan" {
  type = object({
    id                  = string
    resource_group_name = string
  })
}

variable "application_insights" {
  type = object({
    instrumentation_key = string
  })
}

variable "azure_storage" {
  type = object({
    name                      = string
    primary_connection_string = string
  })
}

variable "function_storage" {
  type = object({
    primary_connection_string = string
  })
}

variable "servicebus_namespace" {
  type = object({
    name = string
  })
}

variable "sql_server" {
  type = object({
    name                        = string
    fully_qualified_domain_name = string
  })
}


