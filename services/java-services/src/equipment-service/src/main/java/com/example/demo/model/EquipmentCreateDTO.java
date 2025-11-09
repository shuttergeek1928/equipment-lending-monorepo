package com.example.demo.model;

import jakarta.validation.constraints.Min;
import jakarta.validation.constraints.NotBlank;
import lombok.Getter;
import lombok.Setter;

@Getter
@Setter
public class EquipmentCreateDTO {

    @NotBlank(message = "Equipment name cannot be blank")
    private String equipmentName;

    @NotBlank(message = "Category cannot be blank")
    private String category;

    private String equipmentCondition;

    @Min(value = 1, message = "Total quantity must be at least 1")
    private int totalQuantity;
}