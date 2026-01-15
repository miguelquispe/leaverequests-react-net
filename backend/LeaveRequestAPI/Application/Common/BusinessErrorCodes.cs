namespace LeaveRequestAPI.Application.Common;

public static class BusinessErrorCodes
{
    // Employee related errors
    public const string EMPLOYEE_NOT_FOUND = "EMPLOYEE_NOT_FOUND";
    
    // Date validation errors
    public const string START_DATE_IN_PAST = "START_DATE_IN_PAST";
    public const string INVALID_DATE_RANGE = "INVALID_DATE_RANGE";
    public const string OVERLAPPING_REQUEST = "OVERLAPPING_REQUEST";
    
    // Status validation errors
    public const string INVALID_STATUS_TRANSITION = "INVALID_STATUS_TRANSITION";
    public const string REQUEST_NOT_FOUND = "REQUEST_NOT_FOUND";
    public const string CANNOT_DELETE_APPROVED_REQUEST = "CANNOT_DELETE_APPROVED_REQUEST";
    
    // General business errors
    public const string OPERATION_NOT_ALLOWED = "OPERATION_NOT_ALLOWED";
}
