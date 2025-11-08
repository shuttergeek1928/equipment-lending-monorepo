-- Insert User Types
INSERT INTO public.UserTypes (TypeId, TypeValue)
VALUES
    (1, 'Admin'),
    (2, 'Staff'),
    (3, 'Student')
ON CONFLICT DO NOTHING;

-- Insert Users
INSERT INTO public.Users (
    ID, UserId, UserName, Email, PasswordHash, PasswordSalt, UserType, IsActive
)
VALUES
    ('11111111-1111-1111-1111-111111111111', 1001, 'Admin User', 'admin@example.com',
        'HASHED_PASSWORD_1', 'SALT_1', 1, TRUE),
    ('22222222-2222-2222-2222-222222222222', 1002, 'Staff User', 'staff@example.com',
        'HASHED_PASSWORD_2', 'SALT_2', 2, TRUE),
    ('33333333-3333-3333-3333-333333333333', 1003, 'Student User', 'student@example.com',
        'HASHED_PASSWORD_3', 'SALT_3', 3, TRUE)
ON CONFLICT DO NOTHING;

-- Insert Equipments
INSERT INTO public.Equipments (
    ID, EquipmentName, Catgory, EquipmentCondition, TotalQuantity, AvailableQuantity, IsAvailable
)
VALUES
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'Projector', 'Electronics', 'Good', 5, 4, TRUE),
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'Laptop', 'Electronics', 'New', 10, 9, TRUE),
    ('cccccccc-cccc-cccc-cccc-cccccccccccc', 'Camera', 'Multimedia', 'Fair', 3, 2, TRUE)
RETURNING EquipmentId;

-- NOTE: Run the above first to get EquipmentId values if needed.
-- For demonstration we assume the generated Equipment IDs are 1,2,3

-- Insert Borrow & Return Records
INSERT INTO public.BorrowingsAndReturns (
    ID, EquipmentId, RequesterId, RequestedQuantity, RequestedOn, ReturnDueDate, IsApproved, IsReturned, Notes
)
VALUES
    ('99999999-9999-9999-9999-999999999991', 1, 1003, 1,
        NOW() - INTERVAL '5 days', NOW() + INTERVAL '5 days', TRUE, FALSE, 'Urgent usage'),
        
    ('99999999-9999-9999-9999-999999999992', 2, 1002, 2,
        NOW() - INTERVAL '10 days', NOW() - INTERVAL '2 days', TRUE, TRUE, 'Returned in good condition')
ON CONFLICT DO NOTHING;
