package com.example.demo.model;

import jakarta.persistence.*;
import jakarta.validation.constraints.Min;
import jakarta.validation.constraints.NotBlank;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@Entity
@Table(name = "equipment")
@Getter
@Setter
@NoArgsConstructor
public class Equipment {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @NotBlank
    private String name;

    @NotBlank
    private String category;

    private String condition; // e.g., "New", "Good", "Fair"

    @Min(0)
    private int totalQuantity;

    // This would be managed by a future "borrowing" service
    // For now, we set it equal to totalQuantity
    @Min(0)
    private int availableQuantity;
}