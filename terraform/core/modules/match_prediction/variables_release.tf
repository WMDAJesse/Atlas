// Variables set at release time.

variable "APPLICATION_INSIGHTS_LOG_LEVEL" {
  type = string
}

variable "DATABASE_PASSWORD" {
  type = string
}

variable "DATABASE_USERNAME" {
  type = string
}

variable "DATABASE_SKU_SIZE" {
  type = string
}

variable "DATABASE_MAX_SIZE" {
  type = string
}

variable "IP_RESTRICTION_SETTINGS" {
  type    = list(string)
  default = []
}

variable "MAC_SOURCE" {
  type = string
}

variable "WEBSITE_RUN_FROM_PACKAGE" {
  type = string
}
