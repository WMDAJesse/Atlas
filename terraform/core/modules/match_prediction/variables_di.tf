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
    id                        = string
    name                      = string
    primary_connection_string = string
  })
}

variable "servicebus_namespace" {
  type = object({
    id   = string
    name = string
  })
}

variable "shared_function_storage" {
  type = object({
    primary_access_key = string
    name               = string
  })
}

variable "mac_import_table" {
  type = object({
    name = string
  })
}

variable "servicebus_namespace_authorization_rules" {
  type = object({
    manage     = object({ primary_connection_string = string })
    read-write = object({ primary_connection_string = string })
    read-only  = object({ primary_connection_string = string })
    write-only = object({ primary_connection_string = string })
  })
}

variable "servicebus_topics" {
  type = object({
    alerts        = object({ name = string })
    notifications = object({ name = string })
  })
}

variable "sql_database" {
  type = object({
    name = string
  })
}

variable "sql_server" {
  type = object({
    id                          = string
    name                        = string
    fully_qualified_domain_name = string
  })
}