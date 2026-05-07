function ProductListPage() {
    const products = [
        {
            id: 1,
            name: "Nike Running T-Shirt",
            brand: "Nike",
            category: "T-Shirt",
            price: 29.99,
            status: "Active",
            stock: 120
        },
        {
            id: 2,
            name: "Adidas Training Shorts",
            brand: "Adidas",
            category: "Shorts",
            price: 35.5,
            status: "Active",
            stock: 80
        },
        {
            id: 3,
            name: "Puma Sport Jacket",
            brand: "Puma",
            category: "Jacket",
            price: 79.99,
            status: "Inactive",
            stock: 0
        }
    ];

    return (
        <div>
            <div className="d-flex justify-content-between align-items-center mb-4">
                <div>
                    <h3 className="fw-bold mb-1">Products</h3>
                    <p className="text-muted mb-0">
                        Manage product information, stock and status.
                    </p>
                </div>

                <button className="btn btn-dark">
                    + Add Product
                </button>
            </div>

            <div className="card border-0 shadow-sm">
                <div className="card-body">

                    <div className="d-flex justify-content-between align-items-center mb-3">
                        <h5 className="mb-0 fw-semibold">
                            Product List
                        </h5>

                        <input
                            type="text"
                            className="form-control w-auto"
                            placeholder="Search product..."
                        />
                    </div>

                    <div className="table-responsive">
                        <table className="table table-hover align-middle">
                            <thead className="table-light">
                                <tr>
                                    <th>#</th>
                                    <th>Product</th>
                                    <th>Brand</th>
                                    <th>Category</th>
                                    <th>Price</th>
                                    <th>Stock</th>
                                    <th>Status</th>
                                    <th className="text-end">Actions</th>
                                </tr>
                            </thead>

                            <tbody>
                                {products.map((product, index) => (
                                    <tr key={product.id}>
                                        <td>{index + 1}</td>

                                        <td className="fw-medium">
                                            {product.name}
                                        </td>

                                        <td>{product.brand}</td>

                                        <td>{product.category}</td>

                                        <td>
                                            ${product.price.toFixed(2)}
                                        </td>

                                        <td>{product.stock}</td>

                                        <td>
                                            <span
                                                className={
                                                    product.status === "Active"
                                                        ? "badge bg-success"
                                                        : "badge bg-secondary"
                                                }
                                            >
                                                {product.status}
                                            </span>
                                        </td>

                                        <td className="text-end">
                                            <button className="btn btn-sm btn-outline-primary me-2">
                                                View
                                            </button>

                                            <button className="btn btn-sm btn-outline-dark me-2">
                                                Edit
                                            </button>

                                            <button className="btn btn-sm btn-outline-danger">
                                                Delete
                                            </button>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>

                </div>
            </div>
        </div>
    );
}

export default ProductListPage;