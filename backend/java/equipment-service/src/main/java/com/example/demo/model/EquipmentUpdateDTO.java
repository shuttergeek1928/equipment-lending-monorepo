package com.example.demo.model;

import jakarta.validation.constraints.Min;
import jakarta.validation.constraints.NotBlank;
import lombok.Getter;
import lombok.Setter;

@Getter
@Setter
public class EquipmentUpdateDTO {

    @NotBlank(message = "Equipment name cannot be blank")
    private String equipmentName;

    @NotBlank(message = "Category cannot be blank")
    private String category;

    private String equipmentCondition;

    @Min(value = 0, message = "Total quantity cannot be negative")
    private int totalQuantity;

    @Min(value = 0, message = "Available quantity cannot be negative")
    private int availableQuantity;
}