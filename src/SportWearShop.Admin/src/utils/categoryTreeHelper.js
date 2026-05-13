export function buildCategoryTree(categories) {
    const categoryMap = new Map();

    categories.forEach((category) => {
        categoryMap.set(category.categoryId, {
            ...category,
            children: []
        });
    });

    const rootCategories = [];

    categoryMap.forEach((category) => {
        if (category.parentCategoryId) {
            const parent = categoryMap.get(category.parentCategoryId);

            if (parent) {
                parent.children.push(category);
            }
        } else {
            rootCategories.push(category);
        }
    });

    return rootCategories.sort((a, b) => a.sortOrder - b.sortOrder);
}

export function getCategoryLevel(category, categories) {
    let level = 1;
    let currentParentId = category.parentCategoryId;

    while (currentParentId) {
        const parent = categories.find(
            (item) => item.categoryId === currentParentId
        );

        if (!parent) break;

        level += 1;
        currentParentId = parent.parentCategoryId;
    }

    return level;
}